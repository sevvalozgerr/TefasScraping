using CaseStudy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ScrapeController : ControllerBase
{
    private readonly ScraperService _scraperService;
    private readonly AppDbContext _context;
    private readonly ILogger<ScrapeController> _logger;

    public ScrapeController(ScraperService scraperService, AppDbContext context, ILogger<ScrapeController> logger)
    {
        _scraperService = scraperService;
        _context = context;
        _logger = logger;
    }
    [HttpGet("scrape")]
    public async Task<IActionResult> StartScraping()
    {
        try
        {
            _logger.LogInformation("Starting scraping process");
            await _scraperService.ScrapeData();
            return Ok(new { message = "Scraping completed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during scraping process");
            return StatusCode(500, new { error = "An error occurred during scraping", details = ex.Message });
        }
    }

    [HttpGet("funds")]
    public async Task<IActionResult> GetAllFunds()
    {
        try
        {
            var funds = await _context.Funds
                .Include(f => f.Returns.OrderByDescending(r => r.Id).Take(1))
                .Select(f => new
                {
                    f.Code,
                    f.Name,
                    f.Type,
                    Returns = f.Returns.Select(r => new
                    {
                        r.OneMonthReturn,
                        r.ThreeMonthReturn,
                        r.SixMonthReturn,
                        r.YearToDateReturn,
                        r.OneYearReturn,
                        r.ThreeYearReturn,
                        r.FiveYearReturn
                    }).FirstOrDefault()
                })
                .ToListAsync();

            return Ok(funds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving funds");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("funds/{code}")]
    public async Task<IActionResult> GetFundByCode(string code)
    {
        try
        {
            var fund = await _context.Funds
                .Include(f => f.Returns.OrderByDescending(r => r.Id).Take(1))
                .FirstOrDefaultAsync(f => f.Code == code);

            if (fund == null)
                return NotFound(new { error = $"Fund with code {code} not found" });

            var result = new
            {
                fund.Code,
                fund.Name,
                fund.Type,
                Returns = fund.Returns.Select(r => new
                {
                    r.OneMonthReturn,
                    r.ThreeMonthReturn,
                    r.SixMonthReturn,
                    r.YearToDateReturn,
                    r.OneYearReturn,
                    r.ThreeYearReturn,
                    r.FiveYearReturn
                }).FirstOrDefault()
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fund {Code}", code);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

