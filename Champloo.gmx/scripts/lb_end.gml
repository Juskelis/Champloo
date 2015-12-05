///lb_end( bool connectNav = false )
/*
 * Finishes building the current container,
   and gives focus to the upper-level one.
 * All that with a single stack popping, yes.
 */
 
/*
    Hooks up all of the navigation components in a default manner
*/

var closing = ds_stack_pop(containers_stack);

if(argument_count > 0 && argument[0])
{
    show_debug_message("----HOOKING UP NAVIGATION----");
    with(closing)
    {
        if(is(par_list_container))
        {
            //dealing with single rowed containers
            var count = ds_list_size(items);
            for(var i = 0; i < count; i++)
            {
                var elem = ds_list_find_value(items, i);
                if(!is(par_navigation, elem))
                    continue;
                
                var next = noone;
                j = 1;
                while(j < count && next == noone)
                {
                    var test = ds_list_find_value(items, (i + j)%count);
                    if(is(par_navigation, test))
                    {
                        next = test;
                    }
                    j++;
                }
                if(next == noone) continue;
                
                if(is(obj_horizontal_container))
                {
                    elem.right = next;
                    next.left = elem;
                }
                else
                {
                    elem.down = next;
                    next.up = elem;
                }
                
            }
        }
        else if(is(par_grid_container))
        {
            //dealing with 2D containers
            
            /*
                ASSUMPTIONS:
                    all grid cells contain at most one navigation element
                    grid cells can be empty
                    diagonal navigation is not allowed
                        so for this configuration:
                            o - o
                            - o -
                            o - o
                        the center element would be unreachable
            */
            var rows = ds_grid_height(items);
            var cols = ds_grid_width(items);
            for(var r = 0; r < rows; r++)
            {
                for(var c = 0; c < cols; c++)
                {
                    var elem = ds_grid_get(items, c, r);
                    if(elem == noone || !is(par_navigation, elem))
                        continue;
                    
                    var next_right = noone;
                    var j = 1;
                    while(j < cols && next_right == noone)
                    {
                        var test = ds_grid_get(items, (c + j)%cols, r);
                        if(test != noone && is(par_navigation, test))
                        {
                            next_right = test;
                        }
                        j++;
                    }
                    if(next_right != noone)
                    {
                        elem.right = next_right;
                        next_right.left = elem;
                    }
                    
                    var next_down =  noone;
                    j = 1;
                    while(j < rows && next_down == noone)
                    {
                        var test = ds_grid_get(items, c, (r + j)%rows);
                        if(test != noone && is(par_navigation, test))
                        {
                            next_down = test;
                        }
                        j++;
                    }
                    if(next_down != noone)
                    {
                        elem.down = next_down;
                        next_down.up = elem;
                    }
                }
            }
        }
    }
}