using System.Windows.Controls.Primitives;

namespace WpfControls.MultiSelectList
{
    public interface IMultiSelectCollectionView
    {
        void AddControl(Selector selector);
        void RemoveControl(Selector selector);
    }
}
