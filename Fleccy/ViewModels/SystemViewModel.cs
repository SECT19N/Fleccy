using Hardware.Info;
using System.Collections.ObjectModel;

namespace Fleccy.ViewModels;

public class SystemViewModel {
	private readonly HardwareInfo _hardwareInfo = new();

	public ObservableCollection<SystemTreeNode> SystemNodes { get; } = [];

	public SystemViewModel() {
		LoadSystemData();
	}

	private void LoadSystemData() {
		_hardwareInfo.RefreshBIOSList();

		foreach (BIOS bios in _hardwareInfo.BiosList) {
			SystemTreeNode biosNode = new() {
				Name = "BIOS",
				Children = [
					new() { Name = $"Name: {bios.Name}" },
					new() { Name = $"Version: {bios.Version}" },
					new() { Name = $"Release Date (In UTC): {bios.ReleaseDate}" },
					new() { Name = $"Manufacturer: {bios.Manufacturer}" },
					new() { Name = $"SerialNumber: {bios.SerialNumber}" },
					new() { Name = $"Element ID: {bios.SoftwareElementID}" }
				]
			};

			SystemNodes.Add(biosNode);
		}

		_hardwareInfo.RefreshComputerSystemList();

		foreach (ComputerSystem computerSystem in _hardwareInfo.ComputerSystemList) {
			SystemTreeNode computerSystemNode = new() {
				Name = "Computer System",
				Children = [
					new() { Name = $"Name: {computerSystem.Name}" },
					new() { Name = $"Version: {computerSystem.Version}" },
					new() { Name = $"Vendor: {computerSystem.Vendor}" },
					new() { Name = $"UUID: {computerSystem.UUID}" },
					new() { Name = $"SKUNumber: {(computerSystem.SKUNumber == string.Empty ? "N/A" : computerSystem.SKUNumber)}" },
					new() { Name = $"IdentifyingNumber: {computerSystem.IdentifyingNumber}" }
				]
			};

			SystemNodes.Add(computerSystemNode);
		}

		_hardwareInfo.RefreshMotherboardList();

		foreach (Motherboard mb in _hardwareInfo.MotherboardList) {
			SystemTreeNode mbNode = new() {
				Name = "Motherboard",
				Children = [
					new() { Name = $"Name: {mb.Product}" },
					new() { Name = $"Manufacturer: {mb.Manufacturer}" },
					new() { Name = $"Serial Number: {mb.SerialNumber}" }
				]
			};

			SystemNodes.Add(mbNode);
		}

		_hardwareInfo.RefreshOperatingSystem();

		OS operatingSystem = _hardwareInfo.OperatingSystem;

		SystemTreeNode osNode = new() {
			Name = "Operating System",
			Children = [
				new() { Name = $"Name: {operatingSystem.Name}" },
				new() { Name = $"Version: {operatingSystem.VersionString}" }
			]
		};

		SystemNodes.Add(osNode);
	}
}

public class SystemTreeNode {
	public string Name { get; set; } = "N/A";
	public ObservableCollection<SystemTreeNode> Children { get; set; } = [];
}