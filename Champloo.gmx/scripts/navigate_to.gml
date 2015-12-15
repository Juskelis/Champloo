///navigate_to(next)
//navigates to given child of type par_navigation


//validation that we can navigate to next
var next = argument0;
with(next)
{
    if(!is(par_navigation))
    {
        err("cannot navigate to non-navigation object!");
        exit;
    }
}

with(obj_navigation)
{
    var method = get_method(obj_navigation, "navigate_to");
    script_execute(method, next);
}