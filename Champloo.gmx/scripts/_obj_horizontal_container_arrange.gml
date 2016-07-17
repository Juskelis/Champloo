/*
 * obj_horizontal_container arrange script
 * Arranges all items in a row.
 * Then, it aligns the individual items vertically
   and the whole row horizontally in container's region.
 */

//preparation
var _hmult, _vmult;         //align multiplier
var _iwidth, _iheight;      //items group size

_hmult = components_halign/2;
_vmult = components_valign/2;
_iwidth = 0;
_iheight = 0;

//size calculation
var i, _size, _item;

_size = ds_list_size(items);
for (i=0; i<_size; i++) {
    with (ds_list_find_value(items, i)) {
        _iwidth += width;
        _iheight = max(height, _iheight);
        }
    }

//component resizing, if needed
if (adaptive)
    component_resize(_iwidth, _iheight);

//items arrangement
var _xnext;

_xnext = y + floor(_hmult * (width - _iwidth));
for (i=0; i<_size; i++) {
     with (ds_list_find_value(items, i)) {
        component_shift_to(_xnext, y+floor(_vmult*(other.height-height)) );
        _xnext += width;
        }
    }

show_debug_message(string(width) + " " + string(height));
