using Fleccy.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fleccy.Models;

public class TreeNode(string name) : INotifyPropertyChanged {
	public string Name {
		get => name;
		set {
			if (name != value) {
				name = value;
				OnPropertyChanged();
			}
		}
	}

	public ObservableCollection<TreeNode> Children { get; set; } = [];

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new(propertyName));
}