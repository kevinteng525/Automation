module ApplicationHelper
  def shorten_string(s, length)
    if s==nil
      nil
    elsif s.length > length
      s[0...length] + '...'
    else
      s
    end
  end
  
  def force_build_provider_list
    Provider.get_provider_list_by_category("Build").collect{|p|[p.name,p.id]}
  end
  def force_environment_provider_list
    Provider.get_provider_list_by_category("Environment").collect{|p|[p.name,p.id]}
  end
  def force_test_case_provider_list    
    Provider.get_provider_list_by_category("TestCase").collect{|p|[p.name,p.id]}
  end
  def force_products_list
    Product.get_all_force_products.collect{|p|[p.name,p.id]}
  end
  def force_projects_list
    Project.get_all_force_projects.collect{|p|[p.name,p.id]}
  end
  def force_tasks_list
    AutomationTask.get_all_force_automation_tasks.collect{|t|[t.name, t.id]}
  end
  
  def force_users_list
    User.get_all_force_users.collect{|u|[u.name,u.id]}
  end
  
  def force_test_cases_list
    TestCase.get_all_force_test_cases.collect{|t|[t.name,t.id]}
  end
  
  def force_test_case_name(id)
    TestCase.get_force_test_case_by_id(id).name
  end
  
  def force_product_name(product_id)
    if(product_id)
      Product.get_force_product_by_id(product_id).name
    else
      ''
    end
  end
  
  def force_project_name(project_id)
    if(project_id)
      Project.get_force_project_by_id(project_id).name
    else
      ''
    end
  end
  
  def force_user_name(user_id)
    if(user_id)
      User.get_force_user_by_id(user_id).name 
    else
      ''
    end
  end
  
  def force_user_type_name(user_type)
    User::USER_TYPES[user_type]
  end
  
  def force_user_role_name(user_role)
    User::USER_ROLES[user_role]
  end
  
  def force_build_name(build_id)
    if(build_id)
      Build.get_force_build_by_id(build_id).name
    else
      ''
    end
  end
  
  def force_supported_environment_name(id)
    if(id)
      SupportedEnvironment.get_force_supported_environment_by_id(id).name
    else
      ''
    end
  end
  
  def result_type_name(id)
    TestResult::RESULT_TYPE[id]
  end
  
  def display_class_for_result(result)
    r = ''
    if(result == 'Pass')
      r = 'pass'
    elsif (result == 'Failed')
      r = 'failed'
    elsif (result == 'Time Out')
      r = 'timeout'
    elsif (result == 'Exception')
      r = 'exception'
    elsif (result == 'Not Run')
      r = 'notrun'
    elsif (result == 'Known Product Issue')
      r = 'knownissue'
    elsif (result == 'New Product Issue')
      r = 'newissue'
    elsif (result == 'Environment Issue')
      r = 'environmentissue'
    elsif (result == 'Scripts Issue')
      r = 'scriptsissue'
    elsif (result == 'Common Library Issue')
      r = 'commonlibraryissue'
    end
    r
  end
end