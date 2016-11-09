module SupportedEnvironmentsHelper
  def supported_environment_name(id)
    e = SupportedEnvironment.get_force_supported_environment_by_id(id)
    if(e)
      e.name
    else
      nil
    end
  end
  
  def supported_environment_list
    list = SupportedEnvironment.get_all_force_supported_environments().collect{|p|[p.name,p.id]}
  end
end
