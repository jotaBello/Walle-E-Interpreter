from PIL import Image

def image_to_hex(image_path, file_name):

        img = Image.open(image_path)
        

        img=img.convert("RGB")

        pixels = img.load()
        widht, height = img.size

        
        with open(file_name, "w") as file:
            i=0
            for y in range(height):
                j=0
                for x in range(widht):
                    r, g, b = pixels[x, y]
                    hex_color = "#{:02X}{:02X}{:02X}".format(r, g, b)
                    file.write(f"SetPixel({i},{j},\"{hex_color}\") \n")
                    j+=1 
                i+=1
    



image_path = "Image.png" #CHANGE HERE THE IMAGE NAME

file_name = "File.pw"  #CHANGE HERE THE FILE NAME

image_to_hex(image_path,image_path)