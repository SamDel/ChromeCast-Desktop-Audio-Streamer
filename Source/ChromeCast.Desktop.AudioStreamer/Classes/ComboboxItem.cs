namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    /// <summary>
    /// Used for items in a combo box.
    /// Translations are also supported.
    /// </summary>
    public class ComboboxItem
    {
        public object Value { get; set; }

        public ComboboxItem(object value)
        {
            Value = value;
        }

        /// <summary>
        /// Return the text, translate the supported enums.
        /// </summary>
        public override string ToString()
        {
            if (Value is SupportedStreamFormat || Value is FilterDevicesEnum)
                return Resource.Get(Value.ToString());

            return Value.ToString();
        }
    }
}
