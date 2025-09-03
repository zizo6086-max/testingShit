using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IKinguinSyncProcessService
{
    string StartSyncProcess();
    ProcessStatus? GetProcessStatus(string processId);
    bool CancelProcess(string processId);
    Task StartBackgroundSyncAsync(string processId, string userId);
}
