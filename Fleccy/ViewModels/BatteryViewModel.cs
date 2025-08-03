using Hardware.Info;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fleccy.ViewModels;

public class BatteryViewModel : ViewModelBase {
	private readonly HardwareInfo _hardwareInfo = new();

	public ObservableCollection<BatteryTreeNode> BatteryNodes { get; } = [];

	public BatteryViewModel() {
		LoadBatteryData();
	}

	private void LoadBatteryData() {
		_hardwareInfo.RefreshBatteryList();
		foreach (Battery battery in _hardwareInfo.BatteryList) {
			BatteryTreeNode batteryNode = new() { Name = $"{battery.TimeOnBattery}" };

			batteryNode.Children.Add(new() { Name = $"Status: {battery.BatteryStatusDescription}" });
			batteryNode.Children.Add(new() { Name = $"Charge: {battery.EstimatedChargeRemaining}%" });
			batteryNode.Children.Add(new() { Name = $"Design Capacity: {battery.DesignCapacity} mWh" });
			batteryNode.Children.Add(new() { Name = $"Full Charge Capacity: {battery.FullChargeCapacity} mWh" });
			batteryNode.Children.Add(new() { Name = $"Cycle Count: {battery.ExpectedLife}" });
			
			BatteryNodes.Add(batteryNode);
		}
	}
}

public class BatteryTreeNode {
	public string Name { get; set; } = "N/A";
	public ObservableCollection<BatteryTreeNode> Children { get; set; } = [];
}