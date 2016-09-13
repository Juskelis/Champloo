/*
    obj_navigation navigate_to( next )
*/

var next = argument0;

if(next != component)
{
    if(component != noone)
        component.selected = false;
    
    component = next;
    with(component)
    {
        var method = get_method(object_index, "hover");
        script_execute(method);
    }
}
