using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using Hardware.Info;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Fleccy.ViewModels;

public class MemoryViewModel : ViewModelBase {
	private readonly HardwareInfo _hardwareInfo = new();
	private MemoryTreeNode? _availablePhysicalNode, _availableVirtualNode, _availablePageNode;

	private CancellationTokenSource? _refreshCts;

	public ObservableCollection<MemoryTreeNode> MemoryNodes { get; } = [];

	public MemoryViewModel() {
		LoadMemoryData();

		StartAutoRefresh();
	}

	private void LoadMemoryData() {
		_hardwareInfo.RefreshMemoryStatus();

		// Interesting value here.
		ulong totalVirtualMem = _hardwareInfo.MemoryStatus.TotalVirtual;
		ulong availableVirtual = _hardwareInfo.MemoryStatus.AvailableVirtual;

		ulong totalPageFile = _hardwareInfo.MemoryStatus.TotalPageFile;
		ulong availablePageFile = _hardwareInfo.MemoryStatus.AvailablePageFile;

		ulong totalPhysical = _hardwareInfo.MemoryStatus.TotalPhysical;
		ulong availablePhysical = _hardwareInfo.MemoryStatus.AvailablePhysical;

		MemoryNodes.Add(new MemoryTreeNode("Memory Status") {
			Children = [
				new($"Total Virtual Memory: {totalVirtualMem / 1024 / 1024 / 1024:N0} GB"),
				_availableVirtualNode = new($"Available Virtual Memory: {availableVirtual / 1024 / 1024 / 1024:N0} GB"),
				new($"Total Page File: {totalPageFile / 1024 / 1024:N0} MB"),
				_availablePageNode = new($"Available Page File: {availablePageFile / 1024 / 1024:N0} MB"),
				new($"Total Physical Memory: {totalPhysical / 1024 / 1024:N0} MB"),
				_availablePhysicalNode = new($"Available Physical Memory: {availablePhysical / 1024 / 1024:N0} MB")
			]
		});

		_hardwareInfo.RefreshMemoryList();

		// you never know...
		if (_hardwareInfo.MemoryList.Count == 0) {
			MemoryNodes.Add(new MemoryTreeNode("No memory information available."));
			return;
		} else {
			MemoryTreeNode memoryRootNode = new("Memory Modules: ");

			foreach (Memory memory in _hardwareInfo.MemoryList) {
				MemoryTreeNode memoryNode = new($"Bank: {memory.BankLabel}");

				memoryNode.Children.Add(new($"Size: {memory.Capacity / 1024 / 1024:N0} MB"));
				memoryNode.Children.Add(new($"Speed: {memory.Speed:N0} MHz"));
				memoryNode.Children.Add(new($"Manufacturer: {memory.Manufacturer}"));
				memoryNode.Children.Add(new($"Serial Number: {memory.SerialNumber}"));
				memoryNode.Children.Add(new($"Part Number: {memory.PartNumber}"));
				memoryNode.Children.Add(new($"Form Factor: {memory.FormFactor}"));
				memoryNode.Children.Add(new($"Min/Max Voltage (in mV): {(memory.MinVoltage == 0 ? "N/A" : memory.MinVoltage)} / {(memory.MaxVoltage == 0 ? "N/A" : memory.MaxVoltage)}"));

				memoryRootNode.Children.Add(memoryNode);
			}

			MemoryNodes.Add(memoryRootNode);
		}
	}

	private async void StartAutoRefresh() {
		_refreshCts = new();

		try {
			while (!_refreshCts.Token.IsCancellationRequested) {
				await Dispatcher.UIThread.InvokeAsync(() => UpdateMemoryData());

				await Task.Delay(1000, _refreshCts.Token);
			}
		} catch (TaskCanceledException ex) {
			// todo - Graceful cancellation
			ContentDialog contentDialog = new() {
				Title = "Auto Refresh Stopped",
				Content = $"The auto-refresh task was cancelled: {ex.Message}",
				CloseButtonText = "OK"
			};

			await contentDialog.ShowAsync();
		}
	}

	public void StopAutoRefresh() {
		_refreshCts?.Cancel();
	}

	private void UpdateMemoryData() {
		_hardwareInfo.RefreshMemoryStatus();

		ulong availableVirtual = _hardwareInfo.MemoryStatus.AvailableVirtual;
		ulong availablePageFile = _hardwareInfo.MemoryStatus.AvailablePageFile;
		ulong availablePhysical = _hardwareInfo.MemoryStatus.AvailablePhysical;

		if (_availableVirtualNode != null)
			_availableVirtualNode.Name = $"Available Virtual Memory: {availableVirtual / 1024 / 1024 / 1024:N0} GB";

		if (_availablePageNode != null)
			_availablePageNode.Name = $"Available Page File: {availablePageFile / 1024 / 1024:N0} MB";
		
		if (_availablePhysicalNode != null)
			_availablePhysicalNode.Name = $"Available Physical Memory: {availablePhysical / 1024 / 1024:N0} MB";
	}
}

public class MemoryTreeNode(string name) : INotifyPropertyChanged {
	private string _name = name;

	public string Name {
		get => _name;
		set {
			if (_name != value) {
				_name = value;
				OnPropertyChanged();
			}
		}
	}

	public ObservableCollection<MemoryTreeNode> Children { get; set; } = [];

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}