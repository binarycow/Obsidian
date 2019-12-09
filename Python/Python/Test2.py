from pathlib import Path

rootPath = Path(__file__).parents[0].parents[0].parents[0]

print(rootPath.absolute().as_posix())