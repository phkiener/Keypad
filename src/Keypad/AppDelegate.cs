using Keypad.Core;
using Keypad.Core.Abstractions;

namespace Keypad;

public sealed class AppDelegate : NSApplicationDelegate
{
    private ConnectedDevice? device;
    private NSStatusItem? statusItem;

    public override void DidFinishLaunching(NSNotification notification)
    {
        statusItem = NSStatusBar.SystemStatusBar.CreateStatusItem(NSStatusItemLength.Square);
        statusItem.Button.Image = NSImage.GetSystemSymbol("arcade.stick.console", null);
        
        statusItem.Menu = new NSMenu();
        statusItem.Menu.AddItem(new NSMenuItem("Quit", (_, _) => NSApplication.SharedApplication.Stop(this)));
        
        device = DeviceManger.Connect(DeviceType.StreamDeckXL2022);
        if (device is not null)
        {
            statusItem.Button.Image = NSImage.GetSystemSymbol("arcade.stick.console.fill", null);

            device.SetBrightness(0.5);
            device.KeyPressed += (_, btn) => device.SetImage(btn, new DeviceImage.Color("white"));
            device.KeyReleased += (_, btn) => device.SetImage(btn, new DeviceImage.Color("black"));
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            device?.Dispose();
            device = null;
            
            statusItem?.Dispose();
            statusItem = null;
        }
    }
}
