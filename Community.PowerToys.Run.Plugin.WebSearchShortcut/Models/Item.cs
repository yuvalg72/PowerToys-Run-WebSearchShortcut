using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.WebSearchShortcut.Models
{
  /// <summary>
  /// Key/value record.
  /// </summary>
  public class Item
  {
    /// <summary>
    /// The value.
    /// </summary>
    public string? Name { get; set; }

    public string Url { get; set; } = string.Empty;

    public string Domain {
      get
      {
        return new Uri(Url.Split('?')[0]).GetLeftPart(UriPartial.Authority);
      }
   }

    public string? IconPath { get; set; }

    /// <summary>
    /// When the record was updated.
    /// </summary>
    public DateTime? Updated { get; set; }

    public async Task<bool> DownLoadIcon()
    {
      string iconDirectory = Path.Combine(Main.PluginDirectory, "Images", "Icons");
      if (!Directory.Exists(iconDirectory))
      {
        Directory.CreateDirectory(iconDirectory);
      }
      string iconPath = Path.Combine(Main.PluginDirectory, "Images", "Icons", $"{Name}.png");
      if (!string.IsNullOrEmpty(IconPath) && File.Exists(iconPath))
      {
        IconPath = $@"Images\Icons\{Name}.png";
        return false;
      }

      byte[] icon = await DownloadFaviconAsync();
      if (icon.Length == 0)
      {
        return false;
      }

      try
      {
        await File.WriteAllBytesAsync(iconPath, icon);
        IconPath = $@"Images\Icons\{Name}.png";
      }
      catch (Exception ex)
      {
        Log.Error($"Plugin: {Main.PluginName}\n{ex}", typeof(Item));
        return false;
      }
      return true;
    }

    private async Task<byte[]> DownloadFaviconAsync()
    {
      try
      {
        string faviconUrl = Domain + "/favicon.ico";
        using HttpClient client = new();
        HttpResponseMessage response = await client.GetAsync(faviconUrl);
        if (response.IsSuccessStatusCode)
        {
          return await response.Content.ReadAsByteArrayAsync();
        }
      }
      catch (Exception ex)
      {
        Log.Error($"Plugin: {Main.PluginName}\n{ex}", typeof(Item));
      }
      return [];
    }
  }
}