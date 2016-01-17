/*
 * obj_vertical_container arrange script
 * Arranges all items in a column.
 * Then, it aligns the individual items horizontally
   and the whole column vertically in container's region.
 */

//preparation
var _hmult, _vmult;         //align multipliers
var _iwidth, _iheight;      //items group dimensions

_hmult = components_halign/2;
_vmult = components_valign/2;
_iwidth = 0;
_iheight = 0;

//size calculation
var i, _size, _item;

_size = ds_list_size(items);
for (i=0; i<_size; i++) {
    with (ds_list_find_value(items, i)) {
        _iwidth = max(width, _iwidth);
        _iheight += height;
        }
    }

//component resizing, if needed
if (adaptive)
    component_resize(_iwidth, _iheight);

//items arrangement
var _ynext;

_ynext = y + floor(_vmult * (height - _iheight));
for (i=0; i<_size; i++) {
    with (ds_list_find_value(items, i)) {
        component_shift_to(other.x + floor(_hmult*(other.width-width)), _ynext);
        _ynext += height;
        }
    }