using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IKinguinSyncProcessService
{
    bool IsAnyProcessRunning();
    string StartSyncProcess();
    ProcessStatus? GetProcessStatus(string processId);
    bool CancelProcess(string processId);
    Task StartBackgroundSyncAsync(string processId, string userId);
}
