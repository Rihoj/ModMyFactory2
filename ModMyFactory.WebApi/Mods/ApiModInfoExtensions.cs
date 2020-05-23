//  Copyright (C) 2020 Mathis Rech
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.

namespace ModMyFactory.WebApi.Mods
{
    public static class ApiModInfoExtensions
    {
        /// <summary>
        /// Gets the latest release of the mod and properly resolves extended info
        /// </summary>
        public static ModReleaseInfo GetLatestReleaseSafe(this ApiModInfo info)
        {
            if (info.LatestRelease.HasValue) return info.LatestRelease.Value;

            ModReleaseInfo max = default;
            foreach (var release in info.Releases)
            {
                if (release.Version > max.Version)
                    max = release;
            }
            return max;
        }
    }
}