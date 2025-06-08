using Keypad.Configuration;
using ServiceManagement;

namespace Keypad.MacOS;

public sealed class AppDelegate(KeypadConfig config) : NSApplicationDelegate
{
    private DeviceHub? hub;
    private NSStatusItem? statusItem;

    public override void DidFinishLaunching(NSNotification notification)
    {
        statusItem = NSStatusBar.SystemStatusBar.CreateStatusItem(NSStatusItemLength.Square);
        statusItem.Menu = new NSMenu();

        statusItem.Menu.AddItem(NSMenuItem.SeparatorItem);
        statusItem.Menu.AddItem(
            new NSMenuItem("Launch at login", (sender, _) => ToggleLoginItem(sender))
            {
                State = SMAppService.MainApp.Status is SMAppServiceStatus.Enabled
                    ? NSCellStateValue.On
                    : NSCellStateValue.Off
            });
        
        statusItem.Menu.AddItem(new NSMenuItem("Quit", (_, _) => NSApplication.SharedApplication.Stop(this)));

        hub = new DeviceHub(config, new SendKey());
        
        UpdateImage();
        UpdateDevices();
        hub.OnConnectionsChanged += OnConnectionsChanged;
    }

    private void OnConnectionsChanged(object? sender, EventArgs args)
    {
        InvokeOnMainThread(UpdateImage);
        InvokeOnMainThread(UpdateDevices);
    }

    private void UpdateImage()
    {
        if (statusItem is not null && hub is not null)
        {
            statusItem.Button.Image = hub.IsConnected
                ? NSImage.GetSystemSymbol("arcade.stick.console.fill", null)
                : NSImage.GetSystemSymbol("arcade.stick.console", null);
        }
    }

    private void UpdateDevices()
    {
        if (statusItem?.Menu is not null && hub is not null)
        {
            var previousDevices = statusItem.Menu.Items.TakeWhile(static i => !i.IsSeparatorItem).ToList();
            foreach (var device in previousDevices)
            {
                statusItem.Menu.RemoveItem(device);
            }

            if (hub.ConnectedDevices.Any())
            {
                foreach (var device in hub.ConnectedDevices.OrderByDescending(static d => d))
                {
                    statusItem.Menu.InsertItem(new NSMenuItem(device), 0);
                }
            }
            else
            {
                statusItem.Menu.InsertItem(new NSMenuItem("No devices connected"), 0);
            }
        }
    }

    private void ToggleLoginItem(object? sender)
    {
        var isCurrentlyEnabled = SMAppService.MainApp.Status is SMAppServiceStatus.Enabled;
        if (isCurrentlyEnabled)
        {
            SMAppService.MainApp.Unregister();
        }
        else
        {
            SMAppService.MainApp.Register();
        }

        if (sender is NSMenuItem menuItem)
        {
            menuItem.State = !isCurrentlyEnabled ? NSCellStateValue.On : NSCellStateValue.Off;
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            hub?.Dispose();
            hub = null;
            
            statusItem?.Dispose();
            statusItem = null;
        }
    }
}
