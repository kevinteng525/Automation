module ProductsHelper
  def product_type_options_list (param)    
    param.collect{|t|[t[0], t[1]]}
  end
  def find_index_of_type_options(type_options,value)
    type_options.each{|t| 
      if t[0]==value 
        return t[1] 
        end
        }
  end
  
  def find_name (list,id)
    list.each {|v|
      if v[1]==id
        return v[0]
      end
    }
  end
end
