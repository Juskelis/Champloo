//items arrangement
var i, j;

var num_x_cells = ds_grid_width(items);
var num_y_cells = ds_grid_height(items);

var cell_width = width/num_x_cells;
var cell_height = height/num_y_cells;

for(i = 0; i < num_y_cells; i++) {
    for(j = 0; j < num_x_cells; j++) {
        with(ds_grid_get(items, j, i)) {
            component_shift_to(
                j * cell_width + cell_width/2,
                i * cell_height + cell_height/2
            );
        }
    }
}
