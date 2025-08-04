using Fleccy.ViewModels;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;

namespace Fleccy.Views;

public partial class MainWindow : AppWindow {
	public MainWindow() {
		InitializeComponent();
	}

	private void NavigationView_SelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e) {
		if (DataContext is MainWindowViewModel vm) {
			string? tag = (e.SelectedItem as NavigationViewItem)?.Tag?.ToString();

			switch (tag) {
				case "System":
					vm.NavigateSystem();
					break;
				case "Cpu":
					vm.NavigateCpu();
					break;
				case "Storage":
					vm.NavigateStorage();
					break;
				case "Memory":
					vm.NavigateMemory();
					break;
				case "Battery":
					vm.NavigateBattery();
					break;
			}
		}
	}
}