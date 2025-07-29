using Hardware.Info;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Fleccy.ViewModels;

public class CpuViewModel : ViewModelBase {
	private readonly HardwareInfo _hardwareInfo = new();

	public ObservableCollection<CpuTreeNode> CpuNodes { get; } = [];

	public CpuViewModel() {
		LoadCpuData();
	}

	private void LoadCpuData() {
		_hardwareInfo.RefreshCPUList();

		foreach (CPU cpu in _hardwareInfo.CpuList) {
			CpuTreeNode cpuNode = new() { Name = $"{cpu.Name}" };

			cpuNode.Children.Add(new() { Name = $"Cores: {cpu.NumberOfCores}" });
			cpuNode.Children.Add(new() { Name = $"Threads: {cpu.NumberOfLogicalProcessors}" });
			cpuNode.Children.Add(new() { Name = $"Virtualization: {cpu.VirtualizationFirmwareEnabled}" });

			CpuTreeNode cacheNode = new() {
				Name = "Cache",
				Children = [
					new() { Name = $"L1 Data Cache Size: {cpu.L1DataCacheSize / 1024} KB" },
					new() { Name = $"L1 Instruction Cache Size: {cpu.L1InstructionCacheSize / 1024} KB" },
					new() { Name = $"L2: {cpu.L2CacheSize / 1024} KB" },
					new() { Name = $"L3: {cpu.L3CacheSize / 1024} KB" }
				]
			};

			cpuNode.Children.Add(cacheNode);

			foreach (CpuCore core in cpu.CpuCoreList) {
				CpuTreeNode coreNode = new() { Name = $"Core {core.Name}" };

				cpuNode.Children.Add(coreNode);
			}

			CpuNodes.Add(cpuNode);

			Debug.WriteLine($"Children Count: {cpuNode.Children.Count}");  // Add this in LoadCpuData
		}
	}
}

public class CpuTreeNode {
	public string Name { get; set; } = "N/A";
	public ObservableCollection<CpuTreeNode> Children { get; set; } = [];
}