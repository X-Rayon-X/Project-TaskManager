using System.Collections;
using System.Windows.Forms;

namespace CourseWork_TaskManager
{
    internal class ListComparer : IComparer
    {
        private int _columnindex;

        public int ColumnIndex
        { get { return _columnindex; } set { _columnindex = value; } }

        private SortOrder _sortDirections;

        public SortOrder SortDirections
        { get { return _sortDirections; } set { _sortDirections = value; } }

        public ListComparer()
        {
            _sortDirections = SortOrder.None;
        }

        public int Compare(object x, object y)
        {
            ListViewItem itemx = x as ListViewItem;
            ListViewItem itemy = y as ListViewItem;

            int result;

            switch (_columnindex)
            {
                case 0:
                    result = string.Compare(itemx.SubItems[_columnindex].Text,
                        itemy.SubItems[_columnindex].Text, false);
                    break;
                case 1:
                    double valuex = double.Parse(itemx.SubItems[_columnindex].Text);
                    double valuey = double.Parse(itemy.SubItems[_columnindex].Text);

                    result = valuex.CompareTo(valuey);

                    break;
                default:
                    result = string.Compare(itemx.SubItems[_columnindex].Text,
                        itemy.SubItems[_columnindex].Text, false);

                    break;

            }

            if (_sortDirections == SortOrder.Descending)
            {
                return -result;
            }
            else { return result; }
        }
    }
}
