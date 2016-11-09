require 'rexml/document'
#require 'byebug'
#require 'active_support/core_ext'
include REXML


class XmlHelper
   attr_accessor :ip
   attr_accessor :userdomain
 
   def initialize(path="")
     @doc = path
     @ip =""
     @userdomain=""
   end
   
   #get the DMWeb install machine's ip
   def get_test_agent_ip (machine_name)
     xml = Document.new(File.open(@doc))
     xml.elements.each("config/sutconfig/machines/machine"){|machine| 
       machine.elements.each { |e|
         #byebug
         if machine_name == e.text
            #byebug 
            @ip =  machine.elements["ip"].text
            return @ip
         end
       }    
       
     }   
   end
   
   #get user's domain
   def get_user_domain 
     xml = Document.new(File.open(@doc))
     xml.elements.each("config/sutconfig/domain/name"){|name|        
            @userdomain =  name.text
            return @userdomain
     }   
   end
   
   def xml_to_hash (xml)
      #byebug
      my_hash = Hash.from_xml(xml)
      return my_hash.values.first
   end
end


