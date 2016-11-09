require 'rexml/document'
require 'active_support/core_ext'
include REXML


class XmlHelper
   attr_accessor :ip
 
   def initialize(path="")
     @doc = path
     @ip =""
   end
   def get_test_agent_ip (machine_name)
     xml = Document.new(File.open(@doc))
     xml.elements.each("config/sutconfig/machines/machine"){|machine| 
       machine.elements.each { |e|
         #byebug
         if machine_name == e.text
            #byebug 
            @ip =  machine.elements["ip"].text
         end
       }    
       
     }   
   end
   
   def xml_to_hash (xml)
      #byebug
      my_hash = Hash.from_xml(xml)
      return my_hash.values.first
   end
end


