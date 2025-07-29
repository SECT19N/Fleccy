using Hardware.Info;
using System.Collections.ObjectModel;

namespace Fleccy.ViewModels;

public class StorageViewModel {
	private readonly HardwareInfo _hardwareInfo = new();

	public ObservableCollection<DriveTreeNode> DriveNodes { get; } = [];

	public StorageViewModel() {
		LoadStorageData();
	}

	private void LoadStorageData() {
		_hardwareInfo.RefreshDriveList();

		foreach (Drive drive in _hardwareInfo.DriveList) {
			DriveTreeNode driveNode = new() { Name = $"{drive.Model}" };

			driveNode.Children.Add(new() { Name = $"Size: {drive.Size / 1024 / 1024:N0} MB" });
			driveNode.Children.Add(new() { Name = $"Real Size: {drive.Size:N0} bytes" });
			driveNode.Children.Add(new() { Name = $"Type: {drive.Manufacturer}" });
			driveNode.Children.Add(new() { Name = $"Serial Number: {drive.SerialNumber}" });
			driveNode.Children.Add(new() { Name = $"Description: {drive.Description}" });
			driveNode.Children.Add(new() { Name = $"Firmware Revision: {drive.FirmwareRevision}" });

			DriveTreeNode partitionsNode = new() { Name = "Partitions" };

			foreach (Partition partition in drive.PartitionList) {
				ulong spaceFree = 0, size = 0;
				string fileSystem = string.Empty, caption = string.Empty;
				bool isCompressed = false;

				partition.VolumeList.ForEach(volume => {
					spaceFree += volume.FreeSpace;
					size += volume.Size;
					fileSystem = volume.FileSystem;
					isCompressed = volume.Compressed;
					caption = volume.Caption;
				});

				partitionsNode.Children.Add(new() {
					Name = $"#{partition.Index} {caption}",
					Children = [
						new() { Name = $"Size: {size / 1024 / 1024:N0} MB" },
						new() { Name = $"Free Space: {spaceFree / 1024 / 1024:N0} MB" },
						new() { Name = $"File System: {fileSystem}" },
						new() { Name = $"Is Primary Partition: {partition.PrimaryPartition}" },
						new() { Name = $"Is Bootable: {partition.Bootable}" },
						new() { Name = $"Compressed: {isCompressed}" },
					]
				});
			}

			driveNode.Children.Add(partitionsNode);

			DriveNodes.Add(driveNode);
		}
	}
}

public class DriveTreeNode {
	public string Name { get; set; } = "N/A";
	public ObservableCollection<DriveTreeNode> Children { get; set; } = [];
}