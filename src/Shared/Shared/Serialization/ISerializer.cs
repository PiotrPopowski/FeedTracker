﻿namespace FeedTracker.Shared.Serialization
{
    public interface ISerializer
    {
        string Serialize<T>(T value);
        T? Deserialize<T>(string value);
    }
}
