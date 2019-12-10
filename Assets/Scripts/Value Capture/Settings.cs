using System;

public class Settings
{
    public int NumSamples { get; set; }
    public int SampleFrames { get; set; }
    public DateTime dateTime { get; set; }

    public Settings(int numS, int sFrames, DateTime d)
    {
        NumSamples = numS;
        SampleFrames = sFrames;
        dateTime = d;
    }
}