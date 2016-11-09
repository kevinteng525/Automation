require 'pathname'

class ProjectPath
  
  def self.get_root_path
    project_path = Pathname.new(File.dirname(__FILE__)).realpath.to_s
    array_path = project_path[0,project_path.size-7].split("/")
    len_path = array_path.size
    str=''
    for i in 0..len_path-1
      str += array_path[i]+'\\'
    end
    return str
  end
  
end
