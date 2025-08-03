using Hardware.Info;
using System.Collections.ObjectModel;

namespace Fleccy.ViewModels;

public class MemoryViewModel : ViewModelBase {
	private readonly HardwareInfo _hardwareInfo = new();

	public ObservableCollection<MemoryTreeNode> MemoryNodes { get; } = [];

	public MemoryViewModel() {
		LoadMemoryData();
	}

	private void LoadMemoryData() {
		_hardwareInfo.RefreshMemoryList();

		foreach (Memory memory in _hardwareInfo.MemoryList) {
			MemoryTreeNode memoryNode = new() { Name = $"{memory.BankLabel}" };

			memoryNode.Children.Add(new() { Name = $"Size: {memory.Capacity / 1024 / 1024:N0} MB" });
			memoryNode.Children.Add(new() { Name = $"Speed: {memory.Speed} MHz" });
			memoryNode.Children.Add(new() { Name = $"Manufacturer: {memory.Manufacturer}" });
			memoryNode.Children.Add(new() { Name = $"Serial Number: {memory.SerialNumber}" });
			memoryNode.Children.Add(new() { Name = $"Part Number: {memory.PartNumber}" });
			memoryNode.Children.Add(new() { Name = $"Form Factor: {memory.FormFactor}" });
			memoryNode.Children.Add(new() { Name = $"Min/Max Voltage (in mV): {(memory.MinVoltage == 0 ? "N/A" : memory.MinVoltage)} / {(memory.MaxVoltage == 0 ? "N/A" : memory.MaxVoltage)}" });

			MemoryNodes.Add(memoryNode);
		}
	}
}

public class MemoryTreeNode {
	public string Name { get; set; } = "N/A";
	public ObservableCollection<MemoryTreeNode> Children { get; set; } = [];
}