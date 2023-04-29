﻿using EkoStatRp.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EkoStatRp.Common;

public class PageModelBase<T> : PageModel
{
    protected readonly HttpHelper _httpHelper;
    protected readonly UserHelper _userHelper;
    protected readonly ILogger<T> _logger;
    protected readonly string _apiUrl;

    public PageModelBase(HttpHelper httpHelper, UserHelper userHelper, ILogger<T> logger)
    {
        _httpHelper = httpHelper;
        _userHelper = userHelper;
        _logger = logger;
        _apiUrl = _httpHelper.GetApiUrl();
    }
}