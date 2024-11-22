using CaseStudy;
using CaseStudy.Models;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

public class ScraperService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ScraperService> _logger;

    public ScraperService(AppDbContext context, ILogger<ScraperService> logger)
    {
        _context = context;
        _logger = logger;
    }

    private async Task CleanDatabase()
    {
        try
        {
            //deletes all records from table and relational records
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE fund_returns CASCADE");
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE fund CASCADE");
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cleaning database: {ex.Message}");
            throw;
        }
    }

    public async Task ScrapeData()
    {
        await CleanDatabase();

        IWebDriver driver = null;
        IJavaScriptExecutor js = null;
        try
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");

            driver = new ChromeDriver(options);
            js = (IJavaScriptExecutor)driver;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            driver.Navigate().GoToUrl("https://www.tefas.gov.tr/FonKarsilastirma.aspx");
            await Task.Delay(3000);

            try
            {
                //XPath 
                var getiriTab = wait.Until(d => d.FindElement(By.XPath("//a[contains(text(), 'Getiri')]")));
                getiriTab.Click();
            }
            catch
            {
                //Js
                js.ExecuteScript("document.querySelector('a[href=\"#tabs-1\"]').click();");
            }

            await Task.Delay(2000);

            // Page loop
            bool isNextPageAvailable = true;
            string previousPageHash = string.Empty;

            while (isNextPageAvailable)
            {
                var tableElement = wait.Until(d => d.FindElement(By.CssSelector("table#table_fund_returns")));

                var tableHTML = js.ExecuteScript(@"return document.querySelector('#table_fund_returns').outerHTML;").ToString();

                // Generate hash for current table content
                string currentPageHash = GenerateHash(tableHTML);

                if (currentPageHash == previousPageHash)
                {
                    isNextPageAvailable = false; //no change
                    break;
                }

                previousPageHash = currentPageHash; //update hash

                var tableData = js.ExecuteScript(@"
                const table = document.querySelector('#table_fund_returns');
                if (!table) return [];

                const tbody = table.querySelector('tbody');
                if (!tbody) return [];

                return Array.from(tbody.querySelectorAll('tr')).map(row => {
                   if (row.cells.length < 8) return null;

                   const fundCode = row.cells[0].textContent.trim();
                   if (!fundCode || fundCode.length < 2 || fundCode.length > 4 || 
                       fundCode.includes('PORTFÖY') || 
                       fundCode === 'Tümü' || 
                       fundCode === 'Fon Kodu' ||
                       fundCode.includes('A.Ş.')) {
                       return null;
                   }

                   return Array.from(row.cells).map(cell => cell.textContent.trim());
                }).filter(row => row !== null);
                ");

                var rows = ((IEnumerable<object>)tableData).Cast<IEnumerable<object>>().ToList();

                foreach (var row in rows)
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        var cells = row.Select(c => c.ToString()).ToList();
                        if (cells.Count < 10) continue;

                        var fundCode = cells[0].Trim();
                        if (string.IsNullOrWhiteSpace(fundCode) ||
                            fundCode.Length < 2 ||
                            fundCode.Length > 4 ||
                            fundCode.Contains("PORTFÖY") ||
                            fundCode.Contains("A.Ş.") ||
                            fundCode == "Tümü" ||
                            fundCode == "Fon Kodu")
                        {
                            continue;
                        }

                        var existingFund = await _context.Funds
                            .FirstOrDefaultAsync(f => f.Code == fundCode);

                        if (existingFund == null)
                        {
                            var fund = new Fund
                            {
                                Code = fundCode,
                                Name = cells[1],
                                Type = cells[2]
                            };

                            _context.Funds.Add(fund);
                            await _context.SaveChangesAsync();

                            var fundReturn = new FundReturn
                            {
                                FundId = fund.Id,
                                OneMonthReturn = ParseDecimal(cells[3]),
                                ThreeMonthReturn = ParseDecimal(cells[4]),
                                SixMonthReturn = ParseDecimal(cells[5]),
                                YearToDateReturn = ParseDecimal(cells[6]),
                                OneYearReturn = ParseDecimal(cells[7]),
                                ThreeYearReturn = ParseDecimal(cells[8]),
                                FiveYearReturn = ParseDecimal(cells[9])
                            };

                            _context.FundReturns.Add(fundReturn);
                        }
                        else
                        {
                            existingFund.Name = cells[1];
                            existingFund.Type = cells[2];

                            var fundReturn = new FundReturn
                            {
                                FundId = existingFund.Id,
                                OneMonthReturn = ParseDecimal(cells[3]),
                                ThreeMonthReturn = ParseDecimal(cells[4]),
                                SixMonthReturn = ParseDecimal(cells[5]),
                                YearToDateReturn = ParseDecimal(cells[6]),
                                OneYearReturn = ParseDecimal(cells[7]),
                                ThreeYearReturn = ParseDecimal(cells[8]),
                                FiveYearReturn = ParseDecimal(cells[9])
                            };

                            _context.FundReturns.Add(fundReturn);
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError($"Error processing fund row: {ex.Message}");
                    }
                }

                try
                {
                    //check last page
                    var nextButton = wait.Until(d => d.FindElement(By.XPath("//a[contains(text(), 'Sonraki')]")));
                    if (nextButton != null && nextButton.Displayed)
                    {
                        nextButton.Click();
                        await Task.Delay(2000);
                    }
                    else
                    {
                        isNextPageAvailable = false;
                    }
                }
                catch
                {
                    isNextPageAvailable = false;
                }
            }
        }
        finally
        {
            driver?.Quit();
        }
    }


    private decimal? ParseDecimal(string value)
    {
        try
        {
            //null check
            if (string.IsNullOrWhiteSpace(value) || value == "-")
                return null;

            //cleaning % and space 
            value = value
                .Replace("%", "") 
                .Replace(" ", "") 
                .Trim();

           
            if (value.Contains(",") && value.Contains("."))
            {
                value = value.Replace(".", "").Replace(",", ".");
            }
            else
            {
                value = value.Replace(",", ".");
            }

         
            if (decimal.TryParse(value,
                NumberStyles.Any | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out decimal result))
            {
                return result;
            }

            _logger.LogWarning($"Could not parse decimal value: {value}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error parsing decimal '{value}': {ex.Message}");
            return null;
        }
    }



    //generate hash SHA256.
    private string GenerateHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
