using System.Collections.Generic;
using Worlds.Missions.Dialogs;


public class Get_Random_Device_Mgr
{
    List<DeviceView> selects = new();//被选中的设备列表
    public List<DeviceView> selected_devices => selects;

    public Dictionary<DeviceView, uint> all = new();

    //==================================================================================================


    public void change_selected_device(DeviceView e, bool bl)
    {
        if (bl) selects.Add(e);
        else selects.Remove(e);
    }
}
