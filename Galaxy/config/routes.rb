Galaxy::Application.routes.draw do
  resources :projects do
    resources :supported_environments, :controller => 'product_supported_environments', only: :index
    get :test_result_of_latest_task, :on => :member
    get :test_result_passrate_trend, :on => :member
  end

  resources :providers
  
  resources :subscribers

  resources :diagnostic_logs

  resources :rankings

  resources :releases

  resources :branches

  resources :automation_jobs do
    get :test_case_executions, :on => :member
  end

  get "task_results/:ids", to: "task_results#index"
  
  get "task_results/", to: "task_results#index", as: "task_results"
  
  resources :test_results

  resources :test_case_executions do 
    get :test_result, :on => :member
  end

  get "dashboard/index"
  
  get "help/index"
  
  resources :product_supported_environment_maps

  resources :supported_environments

  resources :automation_tasks do
    get :test_executions, :on => :member
    get :execution_progress, :on => :member
    get :automation_jobs, :on => :member
    get :rerun, :on => :member
    get :report, :on => :member
    put :cancel, :on => :member
  end


  controller :test_depot do
    get 'test_depot/index' => :index
    get 'test_depot/user_created_test_suites_index' => :user_created_test_suites_index
    get 'test_depot/test_suites_of_external_providers_index' => :test_suites_of_external_providers_index
    get 'test_depot/test_plans_of_external_providers_index' => :test_plans_of_external_providers_index
    post 'test_depot/testcases/refresh' => :refreshtestcases
    post 'test_depot/testsuites/refresh' => :refreshtestsuites
  end
  
  
  resources :test_suites
  
  resources :test_suites do
    get :sub_test_suites, :on => :member
    get :sub_test_cases, :on => :member
    get :sub_test_suites_and_cases, :on => :member
  end
  
  resources :test_cases

  #resources :product_environments

  resources :test_environments  

  resources :users do
    get :subscribed_projects, :on => :member
  end  
  
  resources :builds
  
  resources :products do
    resources :builds, :controller => 'product_builds', only: :index    
    get :branches, :on => :member
    get :releases, :on => :member
  end
  
  controller :login do
    get 'login' => :new
    post 'login' => :create
    delete 'logout' => :destroy
  end
  
  root 'dashboard#index'
  # The priority is based upon order of creation: first created -> highest priority.
  # See how all your routes lay out with "rake routes".

  # You can have the root of your site routed with "root"
  # root 'welcome#index'

  # Example of regular route:
  #   get 'products/:id' => 'catalog#view'

  # Example of named route that can be invoked with purchase_url(id: product.id)
  #   get 'products/:id/purchase' => 'catalog#purchase', as: :purchase

  # Example resource route (maps HTTP verbs to controller actions automatically):
  #   resources :products

  # Example resource route with options:
  #   resources :products do
  #     member do
  #       get 'short'
  #       post 'toggle'
  #     end
  #
  #     collection do
  #       get 'sold'
  #     end
  #   end

  # Example resource route with sub-resources:
  #   resources :products do
  #     resources :comments, :sales
  #     resource :seller
  #   end

  # Example resource route with more complex sub-resources:
  #   resources :products do
  #     resources :comments
  #     resources :sales do
  #       get 'recent', on: :collection
  #     end
  #   end

  # Example resource route with concerns:
  #   concern :toggleable do
  #     post 'toggle'
  #   end
  #   resources :posts, concerns: :toggleable
  #   resources :photos, concerns: :toggleable

  # Example resource route within a namespace:
  #   namespace :admin do
  #     # Directs /admin/products/* to Admin::ProductsController
  #     # (app/controllers/admin/products_controller.rb)
  #     resources :products
  #   end
end
