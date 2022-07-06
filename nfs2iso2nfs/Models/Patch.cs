using nfs2iso2nfs.Helpers;

namespace nfs2iso2nfs.Models
{
    public class Patch
    {
        public bool KeepLegit { get; set; }
        public bool MapShoulderToTrigger { get; set; }
        public bool HorizWiimote { get; set; }
        public bool VertWiimote { get; set; }
        public bool Homebrew { get; set; }
        public bool PassThrough { get; set; }
        public bool InstantCC { get; set; }
        public bool NoCC { get; set; }
        public string FwFile { get; set; }
        public Patch(string fwFile)
        {
            FwFile = fwFile;
        }
        public void DoThePatching()
        {
            using var inputIos = new MemoryStream(File.ReadAllBytes(FwFile));                     //copy fw.img into a memory stream
            PatcherHelper.CheckIfRev509(inputIos);

            if (!KeepLegit)
                PatcherHelper.DontKeepLegit(inputIos);
            if (MapShoulderToTrigger)
                PatcherHelper.MapShoulderToTrigger(inputIos);
            if (HorizWiimote || VertWiimote)
                PatcherHelper.EnableWiiRemoteEmulation(inputIos);
            if (HorizWiimote)
                PatcherHelper.EnableHorizontalWiiRemoteEmulation(inputIos);
            if (Homebrew)
                PatcherHelper.EnableProperInputInHomebrew(inputIos);
            if (PassThrough)
                PatcherHelper.WiiMotePassthrough(inputIos);
            if (InstantCC)
                PatcherHelper.InstantCC(inputIos);
            if (NoCC)
                PatcherHelper.NoCC(inputIos);

            using var patchedFile = File.OpenWrite(FwFile);
            inputIos.WriteTo(patchedFile);
        }

    }
}
