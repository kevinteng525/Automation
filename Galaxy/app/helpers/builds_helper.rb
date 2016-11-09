module BuildsHelper

  
  def build_type_list
    Build::BUILD_TYPES.collect{|t|[t,Build::BUILD_TYPES.index(t)]}
  end
  
  def build_status_list
    Build::BUILD_STATUS.collect{|s|[s,Build::BUILD_STATUS.index(s)]}
  end
  
  def product_name_of_build(build)
    Product.get_force_product_by_id(build.product_id.to_i).name
  end
  
  def force_build_type_name(id)
    Build::BUILD_TYPES[id]
  end
  
  def force_build_status_name(status)
    Build::BUILD_STATUS[status]
  end
end
