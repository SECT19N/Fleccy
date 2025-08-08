using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using Hardware.Info;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Fleccy.ViewModels;

public class MemoryViewModel : ViewModelBase {
	private readonly HardwareInfo _hardwareInfo = new();

	// Nodes we'll update dynamically
	private MemoryTreeNode? _availablePhysicalNode;
	private MemoryTreeNode? _availableVirtualNode;
	private MemoryTreeNode? _availablePageFileNode;

	private CancellationTokenSource? _refreshCts;

	public ObservableCollection<MemoryTreeNode> MemoryNodes { get; } = [];

	public MemoryViewModel() {
		LoadMemoryData();
		StartAutoRefresh();
	}

	private void LoadMemoryData() {
		MemoryNodes.Clear();
		_hardwareInfo.RefreshMemoryStatus();

		ulong totalPhysical = _hardwareInfo.MemoryStatus.TotalPhysical;
		ulong availablePhysical = _hardwareInfo.MemoryStatus.AvailablePhysical;

		ulong totalPageFile = _hardwareInfo.MemoryStatus.TotalPageFile;
		ulong availablePageFile = _hardwareInfo.MemoryStatus.AvailablePageFile;

		// Total Virtual Memory is Physical + PageFile
		ulong totalVirtual = totalPhysical + totalPageFile;
		ulong availableVirtual = _hardwareInfo.MemoryStatus.AvailableVirtual;


		MemoryNodes.Add(new("Memory Status") {
			Children = [
				new($"Total Physical Memory: {totalPhysical / 1024 / 1024:N0} MB"),
				_availablePhysicalNode = new($"Available Physical Memory: {availablePhysical / 1024 / 1024:N0} MB"),

				new($"Total Page File (Swap): {totalPageFile / 1024 / 1024:N0} MB"),
				_availablePageFileNode = new($"Available Page File: {availablePageFile / 1024 / 1024:N0} MB"),

				new($"Total Virtual Memory (RAM + Page File): {totalVirtual / 1024 / 1024:N0} MB"),
				_availableVirtualNode = new($"Available Virtual Memory: {availableVirtual / 1024 / 1024:N0} MB")
			]
		});

		_hardwareInfo.RefreshMemoryList();

		if (_hardwareInfo.MemoryList.Count == 0) {
			MemoryNodes.Add(new("No memory module information available."));
			return;
		}

		MemoryTreeNode memoryModulesNode = new("Memory Modules:");

		foreach (Memory memory in _hardwareInfo.MemoryList) {
			MemoryTreeNode memoryNode = new($"Bank: {memory.BankLabel}");

			memoryNode.Children.Add(new($"Size: {memory.Capacity / 1024 / 1024:N0} MB"));
			memoryNode.Children.Add(new($"Speed: {memory.Speed:N0} MHz"));
			memoryNode.Children.Add(new($"Manufacturer: {memory.Manufacturer}"));
			memoryNode.Children.Add(new($"Serial Number: {memory.SerialNumber}"));
			memoryNode.Children.Add(new($"Part Number: {memory.PartNumber}"));
			memoryNode.Children.Add(new($"Form Factor: {memory.FormFactor}"));
			memoryNode.Children.Add(new (
				$"Min/Max Voltage (in mV): " +
				$"{(memory.MinVoltage == 0 ? "N/A" : memory.MinVoltage)}" +
				$" / " +
				$"{(memory.MaxVoltage == 0 ? "N/A" : memory.MaxVoltage)}"));

			memoryModulesNode.Children.Add(memoryNode);
		}

		MemoryNodes.Add(memoryModulesNode);
	}

	private async void StartAutoRefresh() {
		_refreshCts = new();

		try {
			while (!_refreshCts.Token.IsCancellationRequested) {
				await Dispatcher.UIThread.InvokeAsync(UpdateMemoryData);
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

	public void StopAutoRefresh() {
		_refreshCts?.Cancel();
	}

	private void UpdateMemoryData() {
		_hardwareInfo.RefreshMemoryStatus();

		ulong availablePhysical = _hardwareInfo.MemoryStatus.AvailablePhysical;
		ulong availablePageFile = _hardwareInfo.MemoryStatus.AvailablePageFile;
		ulong availableVirtual = _hardwareInfo.MemoryStatus.AvailableVirtual;

		if (_availablePhysicalNode != null)
			_availablePhysicalNode.Name = $"Available Physical Memory: {availablePhysical / 1024 / 1024:N0} MB";

		if (_availablePageFileNode != null)
			_availablePageFileNode.Name = $"Available Page File: {availablePageFile / 1024 / 1024:N0} MB";

		if (_availableVirtualNode != null)
			_availableVirtualNode.Name = $"Available Virtual Memory: {availableVirtual / 1024 / 1024:N0} MB";
	}
}

// MemoryTreeNode with INotifyPropertyChanged for dynamic UI updates
public class MemoryTreeNode(string name) : INotifyPropertyChanged {
	public string Name {
		get => name;
		set {
			if (name != value) {
				name = value;
				OnPropertyChanged();
			}
		}
	}

	public ObservableCollection<MemoryTreeNode> Children { get; set; } = [];

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new(propertyName));
}