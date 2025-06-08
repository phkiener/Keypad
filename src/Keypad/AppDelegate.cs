using Keypad.Configuration;
using Keypad.Core.Abstractions;

namespace Keypad;

public sealed class AppDelegate : NSApplicationDelegate
{
    private DeviceHub? hub;
    private NSStatusItem? statusItem;

    public override void DidFinishLaunching(NSNotification notification)
    {
        statusItem = NSStatusBar.SystemStatusBar.CreateStatusItem(NSStatusItemLength.Square);
        statusItem.Menu = new NSMenu();
        statusItem.Menu.AddItem(NSMenuItem.SeparatorItem);
        statusItem.Menu.AddItem(new NSMenuItem("Quit", (_, _) => NSApplication.SharedApplication.Stop(this)));

        var mockConfig = new KeypadConfig
        {
            Devices =
            [
                new KeypadDeviceConfiguration
                {
                    Type = DeviceType.StreamDeckXL2022,
                    Brightness = 0.5,
                    Keys =
                    [
                        new KeypadKeyConfiguration
                        {
                            Button = new DeviceButton(1, 1),
                            Image = new DeviceImage.Color("red"),
                            Key = new EmulatedKey(EmulatedKey.Keycode.A) { Shift = true }
                        }
                    ],
                }
            ]
        };

        hub = new DeviceHub(mockConfig, new SendKey());
        
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
