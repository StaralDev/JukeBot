import argparse
import pathlib
import json
import re

def configure_json(dat: str):
  datJson = json.loads(dat)

  for k, v in datJson.items():
    if (k == "projectelo.timeline.blind_travel"):
      return v

def main(client: str, loc: str):
  objdir = pathlib.Path(client)
  raw: dict = {}

  assert(objdir.is_dir(), f"Client directory '{client}' was not found!")

  langfiles = [f for f in objdir.iterdir() if f.is_file()]
  for file in langfiles:
    fname = str(file)
    fcont = open(pathlib.Path(fname))
    # -5 for removing .json
    raw[file.name[:-5]] = configure_json(fcont.read())
    fcont.close()

  topath = pathlib.Path(loc)
  topath.parent.mkdir(parents=True, exist_ok=True)
  topath.write_text(json.dumps(raw, indent=2))
    
if __name__ == "__main__":
  p = argparse.ArgumentParser()
  p.add_argument("client", type=str, help="The local path to your specific client folder")
  p.add_argument("loc", type=str, help="The output path for data")
  
  args = p.parse_args()
  main(args.client, args.loc)
