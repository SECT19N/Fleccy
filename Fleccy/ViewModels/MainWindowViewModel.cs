using System.ComponentModel;
using System.Diagnostics;

namespace Fleccy.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged {
	public bool IsPaneOpen { get; set; } = true;

	private object _currentView = new CpuPage { DataContext = new CpuViewModel() };

	public object CurrentView {
		get => _currentView;
		set {
			if (_currentView != value) {
				_currentView = value;
				OnPropertyChanged(nameof(CurrentView));
			}
		}
	}

	public void NavigateCpu() {
		CurrentView = new CpuPage {
			DataContext = new CpuViewModel()
		};
		Debug.WriteLine("Navigated to CPU Page");
	}

	public void NavigateStorage() {
		CurrentView = new StoragePage() {
			DataContext = new StorageViewModel()
		};
		Debug.WriteLine("Navigated to Storage Page");
	}

	public void NavigateMemory() {
		CurrentView = new MemoryPage {
			DataContext = new MemoryViewModel()
		};
		Debug.WriteLine("Navigated to Memory Page");
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	protected virtual void OnPropertyChanged(string propertyName) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}