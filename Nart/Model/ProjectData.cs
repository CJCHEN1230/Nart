using Nart.Model_Object;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nart
{
    public class Projectata : ObservableObject
    {
        private string name = "蔡慧君";
        private string id = "123456";
        private string institution = "成大";
        private  ObservableCollection<BallModel> ballCollection=  new ObservableCollection<BallModel>();

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                SetValue(ref name, value);
            }
        }
        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                SetValue(ref id, value);
            }
        }
        public string Institution
        {
            get
            {
                return institution;
            }
            set
            {
                SetValue(ref institution, value);
            }
        }


        public  ObservableCollection<BallModel> BallCollection
        {
            get
            {
                return ballCollection;
            }
            set
            {
                SetValue(ref ballCollection, value);
            }
        } 

    }
}
