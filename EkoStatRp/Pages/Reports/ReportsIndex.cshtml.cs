using EkoStatLibrary.DtoContainers;
using EkoStatLibrary.Dtos;
using EkoStatLibrary.Enums;
using EkoStatLibrary.Extensions.Common;
using EkoStatLibrary.Extensions.DtoExtensions;
using EkoStatLibrary.Helpers;
using EkoStatLibrary.Models;
using EkoStatRp.Common;
using EkoStatRp.Helpers;
using EkoStatRp.Models;
using Microsoft.AspNetCore.Mvc;

namespace EkoStatRp.Pages.Reports;

[BindProperties]
public class ReportsIndex : PageModelBase<ReportsIndex>
{
    private readonly string _articlesKey;
    private readonly string _tagsKey;
    private readonly TimeHelper _timeHelper;

    public EntriesFilterViewModel FilterViewModel { get; set; } = new();
    public ReportSettingsViewModel Report { get; set; } = new();
    public List<EntryGroupByArticle> EntryGroups { get; set; } = new();
    public List<ReportSegment> Segments { get; set; } = new();

    public ReportsIndex(HttpHelper httpHelper, UserHelper userHelper, ApiHandler apiHandler, TimeHelper timeHelper, ILogger<ReportsIndex> logger)
        : base(httpHelper, userHelper, apiHandler, logger)
    {
        _articlesKey = "Articles";
        _tagsKey = "Tags";
        _timeHelper = timeHelper;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            if (!IsLoggedIn())
                return GoHome();

            var userId = GetUserId();

            FilterViewModel.Articles = await _apiHandler.GetArticlesByUserAsync(userId);
            FilterViewModel.Tags = await _apiHandler.GetTagsByUserAsync(userId);

            SetTempData(_articlesKey, FilterViewModel.Articles);
            SetTempData(_tagsKey, FilterViewModel.Tags);
        }
        catch (Exception ex)
        {
            // TODO: Meddela användaren.
            _logger.LogError(ex, "Fail: Get stuff needed for reports.");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            FilterViewModel.Filter.ArticleIds = Request.Form[Constants.Html.FilterFormArticleIds].ToInts();
            FilterViewModel.Filter.TagIds = Request.Form[Constants.Html.FilterFormTagIds].ToInts();

            var userId = GetUserId();
            var entries = await _apiHandler.GetEntriesFilteredAsync(userId, FilterViewModel.Filter);
            EntryGroups = entries.GroupByArticle();

            var timestamps = entries.Select(e => e.Timestamp).ToList();
            var from = FilterViewModel.Filter.TimestampFrom ?? timestamps.Min();
            var until = FilterViewModel.Filter.TimestampUntil ?? timestamps.Max();
            var timePeriods = GetTimePeriods(from, until);
            Segments = GetSegments(timePeriods, entries);

            FilterViewModel.Articles = GetTempData<List<ArticleResponseDto>>(_articlesKey);
            FilterViewModel.Tags = GetTempData<List<TagResponseDto>>(_tagsKey);
            return Page();
        }
        catch (Exception ex)
        {
            // TODO: Meddela användaren.
            _logger.LogError(ex, "Fail: Generate report.");
            return RedirectToPage();
        }
    }

    private List<TimePeriod> GetTimePeriods(DateTime start, DateTime end)
    {
        var timePeriods = new List<TimePeriod>();

        if (Report.SegmentBy == SegmentSize.None)
            timePeriods.Add(new TimePeriod(start, end));
        else if (Report.SegmentBy == SegmentSize.Week)
            timePeriods = _timeHelper.GetTimePeriodsByWeek(start, end);
        else if (Report.SegmentBy == SegmentSize.Month)
            timePeriods = _timeHelper.GetTimePeriodsByMonth(start, end);
        else if (Report.SegmentBy == SegmentSize.Year)
            timePeriods = _timeHelper.GetTimePeriodsByYear(start, end);

        return timePeriods;
    }

    private List<ReportSegment> GetSegments(List<TimePeriod> timePeriods, List<EntryResponseDto> entries)
    {
        var segments = new List<ReportSegment>();
        foreach (var timePeriod in timePeriods)
        {
            var newSegment = new ReportSegment(timePeriod, entries);
            newSegment.SetCostThreshold(Report.CostLimit, Report.CostLimitType);
            segments.Add(newSegment);
        }
        return segments;
    }
}
