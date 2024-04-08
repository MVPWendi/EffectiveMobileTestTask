using IPTestTask;
using System.Net;

internal class Program
{
    private static readonly Config s_config = new();
    private static readonly AddressProcesser s_adressProcesser = new();
    private static void Main(string[] args)
    {
        // --file-log Z:\Windows\System32\cmd.exe --file-output Z:\Windows\System32\cmd.exe --adress-start 192.168.0.0 --address-mask 255.255.0.0
        var isSetuped = s_config.SetupConfig(args, out string error);
        if(isSetuped == false)
        {
            Console.WriteLine(error);
            return;
        }
        s_adressProcesser.ParseFile(s_config.LogPath);
        s_adressProcesser.PrintMatchingAdresses(s_config.EndAddress, s_config.StartAddress, s_config.TimeStart, s_config.TimeEnd, s_config.OutputPath);
    }
}