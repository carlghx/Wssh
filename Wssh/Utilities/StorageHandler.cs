using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Wssh.Entities;

namespace Wssh.Utilities
{
  /// <summary>
  /// For storing and retrieving profile data from files
  /// Files are saved in the application directory with the extension .wssh
  /// </summary>
  public static class StorageHandler
  {

    /// <summary>
    /// Checks to see if this profile name is already taken
    /// </summary>
    /// <param name="name">Name to check</param>
    /// <returns>True if there is a profile (.wssh file) with this name at the program's location</returns>
    public static bool ProfileExists(string name)
    {
      string fileName = name.ToUpper() + ".wssh";
      string path = App.AppDirectory + fileName;

      return File.Exists(path);
    }

    /// <summary>
    /// Creates a file (.wsshLog extension) with specified error message
    /// </summary>
    /// <param name="msg">Error message to write</param>
    public static void WriteError(string msg)
    {

      string fileName = "Error#" + DateTime.Now.Ticks + ".wsshLog";
      string path = App.AppDirectory + fileName;

      FileStream stream = null;
      StreamWriter writer = null;
      try
      {
        FileStream oStream = new FileStream(path, FileMode.Create);

        writer = new StreamWriter(oStream);
        writer.Write(msg);
      }
      catch
      {
      }
      finally
      {
        if (writer != null)
          writer.Close();
        if (stream != null)
          stream.Close();
      }

    }

    /// <summary>
    /// Saves profile to application folder
    /// </summary>
    /// <param name="profile">Profile object to save</param>
    /// <returns>True if save was successful; false otherwise</returns>
    public static bool SaveProfile(Profile profile)
    {
      string fileName = profile.Name.ToUpper() + ".wssh";
      string path = App.AppDirectory + fileName;

      FileStream stream = null;
      try
      {
        stream = new FileStream(path, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, profile);
      }
      catch
      {
        return false;
      }
      finally
      {
        if (stream != null)
          stream.Close();
      }

      return true;
    }

    /// <summary>
    /// Looks for a given profile name and loads settings into a Profile object
    /// </summary>
    /// <param name="filename">name of profile</param>
    /// <returns>Profile object with all of its saved settings, or null if not found</returns>
    public static Profile Load(string filename)
    {
      string path = String.Format(@"{0}\{1}.wssh", App.AppDirectory, filename.ToUpper());

      FileStream stream = null;

      try
      {
        stream = new FileStream(path, FileMode.Open);

        BinaryFormatter formatter = new BinaryFormatter();
        object o = formatter.Deserialize(stream);

        Profile p = (Profile)o;
        return p;
      }
      catch
      {
      }
      finally
      {
        if (stream != null)
          stream.Close();
      }

      return null;
    }

    /// <summary>
    /// Deletes a profile of the given filename
    /// </summary>
    /// <param name="filename">Name of profile</param>
    /// <returns>True if delete was successful; false otherwise</returns>
    public static bool DeleteProfile(string filename)
    {
      string dir = App.AppDirectory;
      DirectoryInfo dirInfo = new DirectoryInfo(dir);
      FileInfo[] files = dirInfo.GetFiles();

      foreach (FileInfo file in files)
      {
        if (file.Name == filename)
        {
          file.Delete();
          return true;
        }

      }
      return false;
    }

  }
}
