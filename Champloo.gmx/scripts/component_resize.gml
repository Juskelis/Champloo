///component_resize(width,height)
/*
 * Resizes a component to given dimensions.
 * Note that it alone doesn't affect the appearance
   of component unless the appearance depends on
   "width" and "height" variables directly.
 * You might want to create a script with name
   [ObjectName]_on_resize to override default one.
   By default, width and height are simply set
   to values provided. However, upon overriding
   you need to set width and height on your own! 
 */

var _old_width, _old_height;
_old_width = width;
_old_height = height;

//don't do anything if nothing changes
if (_old_width == argument0 && _old_height == argument1)
    exit;

//perform resize itself
script_execute(resize, argument0, argument1);

//don't affect containers if the size ultimately didn't change
if (_old_width == width && _old_height == height)
    exit;

//resizing might affect the layout of parent container
with (contained_by) { layout_changed = true; }

//causes own layout change if the container doesn't adapt
//to items' size and placement
if (is(par_container) && !adaptive)
    layout_changed = true;