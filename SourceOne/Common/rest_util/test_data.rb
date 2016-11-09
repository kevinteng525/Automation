require 'yaml'
class TestData
  attr_accessor :data
  def initialize(path)
    @data = YAML.load(File.open(path))
    
  end   
end


