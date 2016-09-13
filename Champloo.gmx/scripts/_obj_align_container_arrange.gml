/*
 * obj_align_container arrange script
 * Arranges all items in a 3x3 grid.
 * Item position in the grid defines its
   alignment in container's region. E.g.
   item in left-bottom cell will be aligned
   to left-bottom corner of container.
 * Align layout cannot have adaptive size,
   because it's hard to define the size
   that would make sense and be practical
   at the same time.
 */

//items arrangement
var i, j;

for (i=0; i<3; i++) {
for (j=0; j<3; j++) {
    with (ds_grid_get(items, i, j)) {
        component_shift_to(
            other.x + (i*(other.width - width)) div 2,
            other.y + (j*(other.height - height)) div 2
            );
        }
    }
    }
