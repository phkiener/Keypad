using Keypad;

NSApplication.Init();
NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Accessory;
NSApplication.SharedApplication.Delegate = new AppDelegate();
NSApplication.SharedApplication.Run();

return 0;
