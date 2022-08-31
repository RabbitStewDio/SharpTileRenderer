General concept

- separate map data and game state from map rendering
- data elements represented by "tags" (string identifiers) and additional render state (where needed)
- geared towards 2D environments (tile mapping)

Source data assumptions

- game data is grid based (there is a way to query game element at a given position, and 
  game elements have the same size (aka the grid size) or multiples of that (multi-tile 
  structures)
- game data is tagged, carries an identifier that marks the type of entity
- game data is organised in layers (ground, items, actors, decoration etc).

Data flow

- game data transforms into 2D map of tags (f(layer, x,y) => tag, possibly cached for performance.
- matcher systems transform tags into tile-id, possibly taking neighbours into account, possibly cached.
- either of: 
  - tile-id and render state is used for immediate-mode rendering of sprites
  - tile-id and render state is used to configure/update persistant renderer (unity GameObject etc)
