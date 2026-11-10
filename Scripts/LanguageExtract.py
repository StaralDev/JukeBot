import argparse
import pathlib
import json
import os
import re

# IMPORTANT:
# To use this script you need to create a file for your specific hashes! I have a local file called
# langhashes.json, here is a bit of the specification:

# {
#   "minecraft/lang/af_za.json": {
#     "hash": "85d7dc5a58797de3b60faf9648262a52bc8c6a00",
#     "size": 307179
#   },
#   "minecraft/lang/ar_sa.json": {
#     "hash": "2f52323c76963daa760ef04d3c1d763c3bec5639",
#     "size": 644267
#   },
# 
# ... (all of MC's languages)
# 
#  "minecraft/lang/zh_tw.json": {
#     "hash": "f3c08d792383ca9b6e0f9c52dd41f9f8ec8c7ec7",
#     "size": 382487
#   }
# }

# Copy this directly from your indexes or you'll end up with a bunch of garbage data
# By the way idec about big o or whatever this is a quick script

def main(objs: str, hashes: str, loc: str):
  objdir = pathlib.Path(objs)
  hashf = pathlib.Path(hashes)

  langcodeLookup: dict = []

  os.makedirs(loc, exist_ok=True)

  assert(hashf.is_file(), f"Hash file '{hashes}' was not found!")

  hashesRaw = open(hashes)
  hashesJson = json.loads(hashesRaw.read())
  hashesRaw.close()

  for k, v in hashesJson.items():
    destFind = re.search(r"minecraft\/lang\/(.*)\.json", k)
    langcode: str = destFind.group(1)

    hashFull: str = v["hash"]
    hashIntro = hashFull[:2]

    prefixdirs = [f for f in objdir.iterdir() if f.is_dir()]
    for dir in prefixdirs:
      if (not dir.name.startswith(hashIntro)): continue
      indvfile = f"{str(dir)}\\{hashFull}"
      foundpath = pathlib.Path(indvfile)
      assert(foundpath.is_file(), f"Could not find file '{indvfile}' of hash '{hashFull}'")

      langRaw = open(indvfile)
      topath = pathlib.Path(f"{loc}\\{langcode}.json")
      topath.write_text(langRaw.read())
      langRaw.close()

if __name__ == "__main__":
  p = argparse.ArgumentParser()
  p.add_argument("objs", type=str, help="The local path to your specific objects folder")
  p.add_argument("hashes", type=str, help="The local path to a hash-lookup json file")
  p.add_argument("loc", type=str, help="The output path for data")
  
  args = p.parse_args()
  main(args.objs, args.hashes, args.loc)