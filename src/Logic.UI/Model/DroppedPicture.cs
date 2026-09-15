namespace Logic.UI.Model
{
  /// <summary>
  /// A picture which was dropped onto the application, together with the
  /// file format that was determined from its contents.
  /// </summary>
  /// <param name="FileName">
  /// The name the source supplied, with its extension corrected to match
  /// the actual contents. Informational only - on saving the picture is
  /// renamed after the bookmark title.
  /// </param>
  /// <param name="Extension">
  /// The file extension matching the contents, including the leading dot.
  /// </param>
  /// <param name="Bytes">The unaltered file contents.</param>
  public record DroppedPicture(string FileName, string Extension, byte[] Bytes);
}
