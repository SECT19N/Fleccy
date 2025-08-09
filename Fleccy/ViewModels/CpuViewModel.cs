using Avalonia.Threading;
using Fleccy.Models;
using FluentAvalonia.UI.Controls;
using Hardware.Info;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Fleccy.ViewModels;

public class CpuViewModel : ViewModelBase {
	private readonly HardwareInfo _hardwareInfo = new();

	private CancellationTokenSource? _refreshCts;

	public ObservableCollection<TreeNode> CpuNodes { get; } = [];

	public CpuViewModel() {
		LoadCpuData();
		StartAutoRefresh();
	}

	private void LoadCpuData() {
		_hardwareInfo.RefreshCPUList();

		foreach (CPU cpu in _hardwareInfo.CpuList) {
			TreeNode cpuNode = new($"{cpu.Name}");

			cpuNode.Children.Add(new($"Max Clock Speed: {cpu.MaxClockSpeed:N0} MHz"));
			cpuNode.Children.Add(new($"Current Clock Speed: {cpu.CurrentClockSpeed:N0} MHz"));
			cpuNode.Children.Add(new($"Cores: {cpu.NumberOfCores}"));
			cpuNode.Children.Add(new($"Threads: {cpu.NumberOfLogicalProcessors}"));
			cpuNode.Children.Add(new($"Virtualization: {cpu.VirtualizationFirmwareEnabled}"));
			cpuNode.Children.Add(new($"Translation Extension for Virtualization: {cpu.SecondLevelAddressTranslationExtensions}"));
			cpuNode.Children.Add(new($"Socket Designation: {cpu.SocketDesignation}"));
			cpuNode.Children.Add(new($"Manufacturer: {cpu.Manufacturer}"));

			TreeNode cacheNode = new("Cache") {
				Children = [
					new($"L1 Data Cache Size: {cpu.L1DataCacheSize / 1024} KB"),
					new($"L1 Instruction Cache Size: {cpu.L1InstructionCacheSize / 1024} KB"),
					new($"L2: {cpu.L2CacheSize / 1024} KB"),
					new($"L3: {cpu.L3CacheSize / 1024} KB")
				]
			};

			cpuNode.Children.Add(cacheNode);

			foreach (CpuCore core in cpu.CpuCoreList) {
				TreeNode coreNode = new($"Core {core.Name}");

				cpuNode.Children.Add(coreNode);
			}

			CpuNodes.Add(cpuNode);

			Debug.WriteLine($"Children Count: {cpuNode.Children.Count}");  // Add this in LoadCpuData
		}
	}

	private async void StartAutoRefresh() {
		_refreshCts = new();

		try {
			while (!_refreshCts.Token.IsCancellationRequested) {
				await Dispatcher.UIThread.InvokeAsync(UpdateCpuData);
				await Task.Delay(1000, _refreshCts.Token);
			}
		} catch (TaskCanceledException ex) {
			ContentDialog contentDialog = new() {
				Title = "Auto Refresh Stopped",
				Content = $"The memory auto-refresh has been stopped. Reason: {ex.Message}",
				CloseButtonText = "OK"
			};

			Debug.WriteLine($"Auto-refresh stopped: {ex.StackTrace}");

			await contentDialog.ShowAsync();
		}
	}

	private void UpdateCpuData() {
		_hardwareInfo.RefreshCPUList();

		for (int i = 0; i < CpuNodes.Count; i++) {
			CpuNodes[i].Children[1].Name = $"Current Clock Speed: {_hardwareInfo.CpuList[i].CurrentClockSpeed:N0} MHz";
		}
	}
}