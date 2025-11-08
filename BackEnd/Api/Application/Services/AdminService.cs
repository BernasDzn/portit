namespace Api.Application.Services;

using System.Text.RegularExpressions;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;
using Serilog;
using Serilog.Events;

public class AdminService : IAdminService
{
    private readonly ILogger<DockService> _logger;

    public AdminService(ILogger<DockService> logger)
    {
        _logger = logger;
    }
    
    private static readonly string LOG_REGEX =
    @"^\[(?<timestamp>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}) (?<level>[A-Z]+)\]\s*\(\s*\{\s*Id:\s*(?<requestId>\d+)(?:,[^}]*)?\s*\}\s*\)\s*(?<message>.*)$";


    public async Task<IEnumerable<LogDto>> GetLogs(uint max = 10)
    {
        List<LogDto> logDtos = new List<LogDto>();

        string todaysLogfile = $"Logs/audit-{DateTime.UtcNow:yyyyMMdd}.log";
        if (!File.Exists(todaysLogfile))
        {
            throw new FileNotFoundException("Log file not found");
        }

        var regex = new Regex(LOG_REGEX, RegexOptions.Compiled);

        using (var fileStream = new FileStream(todaysLogfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var streamReader = new StreamReader(fileStream))
        {
            string? line;
            while ((line = await streamReader.ReadLineAsync()) != null)
            {
                try
                {
                    var log = new LogDto
                    {
                        Timestamp = regex.Match(line).Groups["timestamp"].Value,
                        RequestId = regex.Match(line).Groups["requestId"].Value,
                        Level = regex.Match(line).Groups["level"].Value,
                        Message = regex.Match(line).Groups["message"].Value
                    };

                    // Only add relevant log events
                    if (
                        (
                            log.RequestId != string.Empty &&
                            Int32.Parse(log.RequestId) >= 1000 &&
                            Int32.Parse(log.RequestId) < 2000
                        ) ||
                        log.Level == "ERROR" ||
                        log.Level == "WARNING" ||
                        log.Level == "CRITICAL"
                    )
                    {
                        logDtos.Add(log);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing log line: {Line}", line);
                }
            }
        }

        return logDtos.OrderByDescending(log => log.Timestamp).Take((int)max);
    }
}